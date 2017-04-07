import lcs = require('longest-common-substring');
import { convertIngredient } from "./weightsAndMeasures";
import { recipesRaw } from './recipes';
const recipes = recipesRaw as Partial<Recipe>[];

//convertIngredient("1oz cheese", "metric");
//convertIngredient("1lb cheese", "metric");
//convertIngredient("10g cheese", "imperial");
convertIngredient("10floz milk", "metric");

interface Recipe {
    name: string,
    description: string,
    cookTime: string,
    cookingMethod: string;
    nutrition: NutritionInformation,
    prepTime: string,
    recipeCategory: string,
    recipeCuisine: string,
    recipeIngredient: string[],
    recipeInstructions: string[],
    recipeYield: string,
    suitableForDiet: string,
    totalTime: string
}

interface NutritionInformation {
    calories: number,
    carbohydrateContent: number,
    cholesterolContent: number,
    fatContent: number,
    fiberContent: number,
    proteinContent: number,
    saturatedFatContent: number,
    servingSize: string,
    sodiumContent: number,
    sugarContent: number,
    transFatContent: number,
    unsaturatedFatContent: number
}

import { Observable } from 'rxjs';
import { Message, CardAction } from 'botframework-directlinejs';
import { ChatConnector } from './Chat';
import { test, testMessage, IntentAction } from './Intent';
import { BrowserBot } from './BrowserBot';

const browserBot = new BrowserBot()

window["browserBot"] = browserBot.botConnection;

const chat = browserBot.chatConnector;

setTimeout(() => chat.send("Let's get cooking!"), 1000);

const intents = {
    instructions: {
        start: /(Let's start|Start|Let's Go|Go|I'm ready|Ready|OK|Okay)\.*/i,
        next: /(Next|What's next|next up|OK|okay|Go|Continue)/i,
        previous: /(go back|back up|previous)/i,
        repeat: /(what's that again|huh|say that again|please repeat that|repeat that|repeat)/i,
        restart: /(start over|start again|restart)/i
    },
    chooseRecipe: /I want to make (?:|a|some)*\s*(.+)/i,
    queryQuantity: /how (?:many|much) (.+)/i,
    askQuestion: /ask/i,
    askYorNQuestion: /yorn/i,
    askChoiceQuestion: /choice/i,
    all: /(.*)/i
}

import { Store, createStore, combineReducers, applyMiddleware, Action } from 'redux';
import { Epic, combineEpics, createEpicMiddleware } from 'redux-observable';

type PartialRecipe = Partial<Recipe>;

const nullAction = { type: null };

interface RecipeState {
    recipe: PartialRecipe,
    lastInstructionSent: number,
    promptKey: string
}

export interface AppState {
    bot: RecipeState
}

type RecipeAction = {
    type: 'Set_Recipe',
    recipe: PartialRecipe
} | {
    type: 'Recipe_Not_Found'
} | {
    type: 'Set_Instruction',
    instruction: number
} | {
    type: 'Set_PromptKey',
    promptKey: string
}

const bot = (
    state: RecipeState = {
        recipe: undefined,
        lastInstructionSent: undefined,
        promptKey: undefined
    },
    action: RecipeAction
) => {
    switch (action.type) {
        case 'Set_Recipe': {
            return {
                ... state,
                recipe: action.recipe,
                lastInstructionSent: undefined
            }
        }
        case 'Set_Instruction': {
            return {
                ... state,
                lastInstructionSent: action.instruction
            }
        }
        case 'Set_PromptKey': {
            return {
                ... state,
                promptKey: action.promptKey
            }
        }
        default:
            return state;
    }
}

const epicSetRecipe: Epic<RecipeAction, AppState> = (action$, store) =>
    action$.ofType('Set_Recipe')
    .flatMap((action: any) =>
        Observable.from([
            `Great, let's make ${action.recipe.name} which ${action.recipe.recipeYield.toLowerCase()}!`,
            "Here are the ingredients:",
            ... action.recipe.recipeIngredient,
            "Let me know when you're ready to go."
        ])
        .zip(Observable.timer(0, 2000), x => x)
    )
    .do(ingredient => chat.send(ingredient))
    .count()
    .mapTo(nullAction);

const store = createStore(
    combineReducers<AppState>({
        bot
    }),
    applyMiddleware(createEpicMiddleware(combineEpics(
        epicSetRecipe
    )))
);

const globalDefaultAction = () => chat.send("I can't understand you. It's you, not me. Get it together and try again.");

const recipeFromName = (name: string) =>
    recipes.find(recipe => recipe.name.toLowerCase() === name.toLowerCase());

const chooseRecipe: IntentAction = groups => {
    const name = groups[1];
    const recipe = recipeFromName(name);
    if (recipe)
        store.dispatch<RecipeAction>({ type: 'Set_Recipe', recipe });
    else
        chat.send(`Sorry, I don't know how to make ${name}. Maybe one day you can teach me.`);
}

const queryQuantity: IntentAction = groups => {
    const ingredientQuery = groups[1].split('');

    const ingredient = store.getState().bot.recipe.recipeIngredient
        .map<[string, number]>(i => [i, lcs(i.split(''), ingredientQuery).length])
        .reduce((prev, curr) => prev[1] > curr[1] ? prev : curr)
        [0];

    chat.send(ingredient);
}

const sayInstruction = (instruction: number) => {
    store.dispatch({ type: 'Set_Instruction', instruction });
    const state = store.getState().bot;
    chat.send(state.recipe.recipeInstructions[state.lastInstructionSent]);
    if (state.recipe.recipeInstructions.length === state.lastInstructionSent + 1)
        chat.send("That's it!");
}

const mustChooseRecipe = () => chat.send("First please choose a recipe");

// implements Prompts

import { ChoiceLists, RespondersFactory, Prompt } from './Prompt';

const recipeChoiceLists: ChoiceLists = {
    'Cheeses': ['Cheddar', 'Wensleydale', 'Brie', 'Velveeta']
}

const recipeResponders: RespondersFactory<AppState> = prompt => ({
    'Favorite_Color': message => {
        if (message.text.toLowerCase() === 'blue') {
            chat.send("That is correct!");
            return true;
        }
        chat.send("Nope, try again");
        return false;
    },
    'Favorite_Cheese': prompt.choiceResponder('Cheeses', choice => {
        if (choice) {
            if (choice === 'Velveeta')
                chat.send('Ima let you finish but FYI that is not really cheese.');
            else
                chat.send("Interesting.");
            return true;
        }
        chat.send("Frankly we don't handle this well.");
        return false;
    }),
    'Like_Cheese': prompt.confirmResponder(confirmed => {
        if (confirmed)
            chat.send('That is correct.');
        else
            chat.send("That is incorrect.");
        return true;
    }),
})

const prompt = new Prompt(chat, store, recipeChoiceLists, recipeResponders);

chat.activity$
.filter(activity => activity.type === 'message')
.subscribe((message: Message) => {
    const state = store.getState().bot;

    if (prompt.respond(message))
        return;

    // Test questions
    if (testMessage(message, [
        test(intents.askQuestion, _ => prompt.text('Favorite_Color', "What is your favorite color?")),
        test(intents.askYorNQuestion, _ => prompt.confirm('Like_Cheese', "Do you like cheese?")),
        test(intents.askChoiceQuestion, _ => prompt.choice('Favorite_Cheese', 'Cheeses', "What is your favorite cheese?"))
    ]))
        return;

    // First priority is to choose a recipe
    if (!state.recipe) {
        testMessage(message, [
            test(intents.chooseRecipe, chooseRecipe),
            test([
                intents.queryQuantity,
                intents.instructions.start,
                intents.instructions.restart
            ], mustChooseRecipe),
            test(intents.all, chooseRecipe)
        ], globalDefaultAction);
        return;
    }
    
    // These can happen any time there is an active recipe
    if (testMessage(message, [
            test(intents.queryQuantity, queryQuantity),
            // TODO: conversions go here
    ])) {
        return;
    }

    // Start instructions
    if (state.lastInstructionSent === undefined) {
        if (testMessage(message, test(intents.instructions.start, () => sayInstruction(0))))
            return;
    }

    // Navigate instructions
    testMessage(message, [
        test(intents.instructions.next, () => {
            const nextInstruction = state.lastInstructionSent + 1;
            if (nextInstruction < state.recipe.recipeInstructions.length)
                sayInstruction(nextInstruction);
            else
                chat.send("That's it!");
        }),
        test(intents.instructions.repeat, () => sayInstruction(state.lastInstructionSent)),
        test(intents.instructions.previous, () => {
            const prevInstruction = state.lastInstructionSent - 1;

            if (prevInstruction >= 0)
                sayInstruction(prevInstruction);
            else
                chat.send("We're at the beginning.");
        }),
        test(intents.instructions.restart, () => sayInstruction(0))
    ], globalDefaultAction);
});
