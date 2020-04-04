import React from 'react';
import logo from './logo.svg';
import { Header } from './Header';
import { HomePage } from './HomePage';
/** @jsx jsx */
import { css, jsx } from '@emotion/core';
import { fontFamily, fontSize, gray2 } from './Styles';

function App() {
  const unused = 'something';
  return (
    <div 
       css={css` 
         font-family: ${fontFamily}; 
         font-size: ${fontSize}; 
         color: ${gray2}; 
        `}
    >
      <Header />
      <HomePage />
    </div>
  );
}

export default App;
