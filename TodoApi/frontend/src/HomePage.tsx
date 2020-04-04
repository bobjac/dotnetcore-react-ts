import React from 'react';
/** @jsx jsx */
import { css, jsx } from '@emotion/core';
import { PrimaryButton } from './Styles';
import { QuestionList } from './QuestionList';
import { getUnansweredQuestions, QuestionData } from './QuestionsData';
import { Page } from './Page'
import { PageTitle } from './PageTitle'


export const HomePage = () => (
    <Page>
    <div
    css={css`
      margin: 50px auto 20px auto;
      padding: 30px 20px;
      max-width: 600px;
    `}
    >
        <div
            css={css`
            display: flex;
            align-items: center;
            justify-content: space-between;
            `}
        >
 
            <h2
            css={css`
                font-size: 15px;
                font-weight: bold;
                margin: 10px 0px 5px;
                text-align: center;
                text-transform: uppercase;
            `}
            >
            Unanswered Questions
            </h2>
            <PageTitle>Unanswered Questions</PageTitle>
            <PrimaryButton>Ask a question</PrimaryButton>
        </div>
        <QuestionList data={getUnansweredQuestions()} />
    </div>
    </Page>
);

const renderQuestion = (question: QuestionData) =>
    <div>{question.title}</div>;