import React, { Component } from 'react';
import MessageList from './MessageList'
import UserInput from './UserInput'
import Header from './Header'

const ChatWindow = (props) => {
    const onMessageReceived = async (message) => {
        //messages: [...this.state.messages, message]
    }

    const onUserInputSubmit = async (message) => {
        props.onUserInputSubmit(message);
    }

    let messageList = props.messageList || [];
    let classList = [
        "app-chat-window",
        (props.isOpen ? "opened" : "closed")
    ];

    return (
        <div className={classList.join(' ')}>
            <Header
                teamName={props.agentProfile.teamName}
                imageUrl={props.agentProfile.imageUrl}
                onClose={props.onClose}
            />
            <MessageList
                messages={messageList}
                imageUrl={props.agentProfile.imageUrl}
            />
            <UserInput onSubmit={onUserInputSubmit.bind(this)} />
        </div>
    );
}
export default ChatWindow;
