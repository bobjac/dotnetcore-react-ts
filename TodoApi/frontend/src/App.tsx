import React from 'react';
import { BrowserRouter, Route, Redirect, Switch } from 'react-router-dom';
import logo from './logo.svg';
import { Header } from './Header';
import { HomePage } from './HomePage';
import { AskPage } from './AskPage';
import { SearchPage } from './SearchPage';
import { SignInPage } from './SignInPage';
import { NotFoundPage } from './NotFoundPage';
/** @jsx jsx */
import { css, jsx } from '@emotion/core';
import { fontFamily, fontSize, gray2 } from './Styles';

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
          <Route path="/ask" component={AskPage} />
          <Route path="/signin" component={SignInPage} />
          <Route component={NotFoundPage} />
        </Switch>
      </div>
    </BrowserRouter>
  );
}

export default App;
