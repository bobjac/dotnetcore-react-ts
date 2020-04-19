import React, { FC, useState, createContext } from 'react';
import { PrimaryButton, gray5, gray6 } from './Styles';
/** @jsx jsx */
import { css, jsx } from '@emotion/core';

export interface Values {
    [key: string]: any;
}

interface FormContextProps {
    values: Values;
    setValue?: (fieldName: string, value: any) => void;
}
export const FormContext = createContext<FormContextProps>({
    values: {},
});

type Validator = (value: any, args?: any) => string;

export const required: Validator = (value: any) : string => 
    value === undefined || 
    value === null || 
    value === '' ? 'This must be populated' : '';

export const minLength: Validator = (
    value: any,
    length: number,
): string =>
    value && value.length < length ? `This must be at least ${length} characters` : '';

interface Validation {
    validator: Validator;
    arg?: any;
}

interface ValidationProp {
    [key: string]: Validation | Validation[];
}

interface Props {
    submitCaption?: string;
    validationRules?: ValidationProp;
}

export const Form: FC<Props> = ({ 
    submitCaption, 
    children,
    validationRules
}) => {
    const [values, setValues] = useState<Values>({});
    return (
        <FormContext.Provider
            value={{
                values,
                setValue: (fieldName: string, value: any) => {
                    setValues({...values, [fieldName]: value});
                },
            }}
        >
        <form noValidate={true}>
            <fieldset
                css={css`
                    margin: 10px auto 0 auto;
                    padding: 30px;
                    width: 350px;
                    background-color: ${gray6};
                    border-radius: 4px;
                    border: 1px solid ${gray5};
                    box-shadow: 0 3px 5px 0 rgba(0, 0, 0, 0.16);
                `}
            >
                {children}
                <div
                    css={css`
                    margin: 30px 0px 0px 0px;
                    padding: 20px 0px 0px 0px;
                    border-top: 1px solid ${gray5};
                    `}
                >
                    <PrimaryButton type="submit">
                    {submitCaption}
                    </PrimaryButton>
                </div>
                </fieldset>
        </form>
        </FormContext.Provider>
    );
};