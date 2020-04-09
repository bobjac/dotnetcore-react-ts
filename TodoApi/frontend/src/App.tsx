import React, { lazy, Suspense } from 'react';
import { BrowserRouter, Route, Redirect, Switch } from 'react-router-dom';
import logo from './logo.svg';
import { Header } from './Header';
import { HomePage } from './HomePage';
import { SearchPage } from './SearchPage';
import { SignInPage } from './SignInPage';
import { NotFoundPage } from './NotFoundPage';
import { QuestionPage } from './QuestionPage';
/** @jsx jsx */
import { css, jsx } from '@emotion/core';
import { fontFamily, fontSize, gray2 } from './Styles';

const AskPage = lazy(() => import('./AskPage'));

function App() {
  const unused = 'something';
  return (
    <BrowserRouter>
      <div 
        css={css` 
          font-family: ${fontFamily}; 
          font-size: ${fontSize}; 
          color: ${gray2}; 
          `}
      >
        <Header />
        <Switch>
          <Redirect from="/home" to="/" />
          <Route exact path="/" component={HomePage} /> 
          <Route path="/search" component={SearchPage} />
          <Route path="/ask">
            <Suspense
              fallback={
                <div
                  css={css`
                    margin-top: 100px;
                    text-align: center;
                  `}
                >
                  Loading....
                </div>
              }
            >
              <AskPage />
            </Suspense>
          </Route>
          <Route path="/signin" component={SignInPage} />
          <Route path="/questions/:questionId" component={QuestionPage} />
          <Route component={NotFoundPage} />
        </Switch>
      </div>
    </BrowserRouter>
  );
}

export default App;
