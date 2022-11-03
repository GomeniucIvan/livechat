import React, { Component } from 'react';
import { IFrame } from '../utils/IFrame';
import Launcher from './Launcher';

class LauncherWrapper extends Component {
    constructor(props) {
        super(props);
        this.state = {
            homeModel: {},
            loading: false,
            messageList: [],
            newMessagesCount: 0,
            isOpen: false
        };
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
        return (
            <div className="app-laucher-container">
                <Launcher
                    agentProfile={{
                        teamName: 'react-live-chat',
                        imageUrl: 'https://a.slack-edge.com/66f9/img/avatars-teams/ava_0001-34.png'
                    }}
                    onMessageWasSent={this._onMessageWasSent.bind(this)}
                    messageList={this.state.messageList}
                    newMessagesCount={this.state.newMessagesCount}
                    handleClick={this._handleClick.bind(this)}
                    isOpen={this.state.isOpen}
                />
            </div>
        )
    }
}

export default LauncherWrapper;
