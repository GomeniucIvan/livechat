import React, {Component} from 'react'
import messageHistory from './messageHistory';
import TestArea from './TestArea';
import Header from './Header';
import Footer from './Footer';
import monsterImgUrl from "./assets/monster.png";
import { get } from "../utils/HttpClient";
import './assets/styles'

export default class Home extends Component {
    constructor() {
        super();
        this.state = {
            messageList: messageHistory,
            newMessagesCount: 0,
            isOpen: false,
            model: {},
            loading: false
        };
    }

    async populateComponent() {
        //const data = await get(`/api/home`);
        //this.setState({ model: data, loading: false });
    }

    componentDidMount() {
        this.populateComponent();
    }

    _onMessageWasSent(message) {
        this.setState({
            messageList: [...this.state.messageList, message]
        })
    }

    _sendMessage(text) {
        if (text.length > 0) {
            const newMessagesCount = this.state.isOpen ? this.state.newMessagesCount : this.state.newMessagesCount + 1
            this.setState({
                newMessagesCount: newMessagesCount,
                messageList: [...this.state.messageList, {
                    author: 'them',
                    type: 'text',
                    data: { text }
                }]
            })
        }
    }

    _handleClick() {
        this.setState({
            isOpen: !this.state.isOpen,
            newMessagesCount: 0
        })
    }

    render() {
        return <div>
            <Header />
            <TestArea
                onMessage={this._sendMessage.bind(this)}
            />
            <img className="demo-monster-img" src={monsterImgUrl} />
            <Footer />
        </div>
    }
}
