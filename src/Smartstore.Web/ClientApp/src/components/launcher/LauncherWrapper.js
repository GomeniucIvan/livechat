import React, { useEffect, useState } from 'react';
import { postLauncher } from '../utils/HttpClient';
import Launcher from './Launcher';

//todo redux
const LauncherWrapper = (props) => {
    let [messageList, setMessageList] = useState([]);
    let [newMessagesCount, setNewMessagesCount] = useState(0);
    let [isOpen, setIsOpen] = useState(false);
    let [loading, setLoading] = useState(true);

    useEffect(() => {
        const PopulateComponent = async () => {

            let response = await postLauncher(`/api/launcher/messages`);

            if (response && response.IsValid) {
                const messagesDb = response.Data;
                setMessageList(messagesDb);
            }
            setLoading(false);
        }
        if (loading) {
            PopulateComponent();
        }
    });

    const _onMessageWasSent = async (message) => {
        //let messages = [...messageList, message];
        //setNewMessagesCount(messages);
    }

    const _sendMessage = async (text) => {
        if (text.length > 0) {
            const newMessagesCount = isOpen ? newMessagesCount : newMessagesCount + 1;
            setNewMessagesCount(newMessagesCount);

            //let messages = [...messageList, {
            //    author: 'them',
            //    type: 'text',
            //    data: { text }
            //}];
            //(messages)
        }
    }

    const _handleClick = async () => {
        setIsOpen(true);
        setNewMessagesCount(0);
    }

    return (
        <div className="app-laucher-container">
            <Launcher
                agentProfile={{
                    teamName: 'react-live-chat',
                    imageUrl: 'https://a.slack-edge.com/66f9/img/avatars-teams/ava_0001-34.png'
                }}
                onMessageWasSent={_onMessageWasSent.bind(this)}
                messageList={messageList}
                newMessagesCount={newMessagesCount}
                handleClick={_handleClick.bind(this)}
                isOpen={isOpen}
            />
        </div>
    )
}

export default LauncherWrapper;
