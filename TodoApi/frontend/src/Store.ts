import { Action } from 'redux';
import { QuestionData } from './QuestionsData';

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

