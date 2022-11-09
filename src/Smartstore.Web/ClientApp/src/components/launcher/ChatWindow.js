import React from 'react';
import MessageList from './MessageList'
import UserInput from './UserInput'
import Header from './Header'
import { Loading } from './../utils/Loading'

const ChatWindow = (props) => {
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
            {props.isOpen &&
                <MessageList
                    messages={messageList}
                    imageUrl={props.agentProfile.imageUrl}
                />
            }
            {!props.isOpen &&
                <Loading />
            }
            <UserInput onSubmit={onUserInputSubmit.bind(this)} />
        </div>
    );
}
export default ChatWindow;
