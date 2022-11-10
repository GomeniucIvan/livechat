import React, { Component } from 'react';
import Document from './components/app/shared/Document';
import { Loading } from './components/utils/Loading';
import './components/app/assets/scss/app.scss'
import 'bootstrap/dist/js/bootstrap.min.js'
import { applyMiddleware, compose, createStore } from 'redux';
import { Provider } from 'react-redux';
import thunk from 'redux-thunk';
import { appReducer } from './components/utils/redux/AppReducer';

const store = createStore(appReducer, compose(
    applyMiddleware(
        thunk
    ),
    //window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__()
));

export default class App extends Component {
    constructor(props) {
        super(props);
        this.state = {
            loading: false,
        };
    }

    render() {
        return (
            <Provider store={store}>
                {this.state.loading &&
                    <Loading />
                }

                {!this.state.loading &&
                    <>
                        <Document />
                    </>
                }
            </Provider>
        )
    }
}
