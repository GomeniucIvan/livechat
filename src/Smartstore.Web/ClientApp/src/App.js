import React, { Component } from 'react';
import Document from './components/app/shared/Document';
import { Loading } from './components/utils/Loading';
import './components/app/assets/scss/app.scss'
import 'bootstrap/dist/js/bootstrap.min.js'

export default class App extends Component {
    constructor(props) {
        super(props);
        this.state = {
            loading: false,
        };
    }

    render() {
        return (
            <>
                {this.state.loading &&
                    <Loading />
                }

                {!this.state.loading &&
                    <>
                        <Document />
                    </>
                }
            </>
        )
    }
}
