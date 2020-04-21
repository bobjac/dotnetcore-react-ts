import { 
    Action, 
    ActionCreator, 
    Dispatch, 
    Reducer, 
    combineReducers,
    Store,
    createStore,
    applyMiddleware
} from 'redux';
import { QuestionData, getUnansweredQuestions, postQuestion, PostQuestionData } from './QuestionsData';
import thunk, { ThunkAction } from 'redux-thunk';

interface QuestionState {
    readonly loading: boolean;
    readonly unanswered: QuestionData[] | null;
    readonly postedResult?: QuestionData;
}

export interface AppState {
    readonly questions: QuestionState;
}

const initialQuestionState: QuestionState = {
    loading: false,
    unanswered: null
};

export interface GettingUnansweredQuestionsAction 
    extends Action<'GettingUnansweredQuestions'> {

}

export interface GotUnansweredQuestionsAction
    extends Action<'GotUnansweredQuestions'> {
        questions: QuestionData[];
}

export interface PostedQuestionAction
    extends Action<'PostedQuestion'> {
        result: QuestionData | undefined;
}

type QuestionActions = 
    | GettingUnansweredQuestionsAction
    | GotUnansweredQuestionsAction
    | PostedQuestionAction;

export const getUnansweredQuestionsActionCreator: 
    ActionCreator<ThunkAction<Promise<void>, QuestionData[], null, GotUnansweredQuestionsAction>> = () => {
    return async (dispatch: Dispatch) => {
        const gettingUnansweredQuestionsAction: GettingUnansweredQuestionsAction = {
            type: 'GettingUnansweredQuestions'
        };
        dispatch(gettingUnansweredQuestionsAction);
        const questions = await getUnansweredQuestions();
        const gotUnansweredQuestionAction: GotUnansweredQuestionsAction = {
            questions,
            type: 'GotUnansweredQuestions'
        };
        dispatch(gotUnansweredQuestionAction);
        // TODO - dispatch

    };
}

export const PostedQuestionActionCreator: 
    ActionCreator<ThunkAction<Promise<void>, QuestionData, PostQuestionData, PostedQuestionAction>> = 
    (question: PostQuestionData) => {
    return async (dispatch: Dispatch) => {
        const result = await postQuestion(question);
        const postedQuestionAction: PostedQuestionAction = {
            type: 'PostedQuestion',
            result
        };
        dispatch(postedQuestionAction);
    };
};

export const clearPostedQuestionActionCreator: 
    ActionCreator<PostedQuestionAction> = () => {
    const postedQuestionAction: PostedQuestionAction = {
        type: 'PostedQuestion',
        result: undefined,
    };
    return postedQuestionAction;
};

const questionsReducer: Reducer<QuestionState, QuestionActions> = (
    state = initialQuestionState,
    action
) => {
    switch (action.type) {
        case 'GettingUnansweredQuestions': {
            return {
                ...state,
                unanswered: null,
                loading: true
            };
        }
        case 'GotUnansweredQuestions' {
            return {
                ...state,
                unanswered: action.questions,
                loading: false
            };
        }
        case 'PostedQuestion': {
            return {
                ...state,
                unanswered: action.result ? (state.unanswered || []).concat(action.result) : state.unanswered,
                postedResult: action.result
            };
        }
        default: {
            neverReached(action);
        }
    }
    return state;
};

const neverReached = (never: never) => {};

const rootReducer = combineReducers<AppState>({
    questions: questionsReducer
});

export function configureStore(): Store<AppState> {
    const store = createStore(
        rootReducer,
        undefined,
        applyMiddleware(thunk)
    );
    return store;
}