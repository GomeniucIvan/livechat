import React, { useEffect, useRef, useState } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { postLauncher } from '../utils/HttpClient';
import Launcher from './Launcher';

let messageArrayList = []; //todo ?! setMessageList reload component!!

const LauncherWrapper = (props) => {
    let [messageList, setMessageList] = useState([]);
    let [newMessagesCount, setNewMessagesCount] = useState(0);
    let [isOpen, setIsOpen] = useState(false);
    let [loading, setLoading] = useState(true);
    const msgListScrollRef = useRef(null);

    useEffect(() => {
        const connection = new HubConnectionBuilder()
            .withUrl("/chatHub")
            .withAutomaticReconnect()
            .configureLogging(LogLevel.None)
            .build();

        connection.start().catch(function (err) {
            console.log('Hub Error' + err);
        });

        connection.on(`guest_1_new_message`, function (message) {
            //const incMessage = {
            //    Id: message.id,
            //    CompanyCustomerId: message.companyCustomerId,
            //    CompanyGuestCustomerId: message.companyGuestCustomerId,
            //    CompanyId: message.companyId,
            //    CreatedOnUtc: message.createdOnUtc,
            //    IconUrl: message.iconUrl,
            //    Message: message.message,
            //    Sent: message.sent
            //}

            //setMessageList([...messageList, incMessage]);
            //messageArrayList = messageList;

            PopulateComponent();
        });

        const PopulateComponent = async () => {
            let response = await postLauncher(`/api/launcher/messages`);
            if (response && response.IsValid) {
                setMessageList(response.Data);
                messageArrayList = response.Data;

                await scrollMessageList();
            }
            setLoading(false);
        }
        PopulateComponent();
    }, []);

    const onGuestSendMessage = async (message) => {
        setMessageList([...messageList, message]);

        await scrollMessageList();
    }

    const handleIconClick = async () => {
        setIsOpen(!isOpen);
        if (isOpen) {
            setNewMessagesCount(0);
        }
        await scrollMessageList();
    }

    const scrollMessageList = async () => {
        setTimeout(() => {
            if (msgListScrollRef.current) {
                msgListScrollRef.current.scrollTop = msgListScrollRef.current.scrollHeight;
            }
        }, 50);
    }

    return (
        <div className="app-laucher-container">
            <Launcher
                agentProfile={{
                    teamName: 'react-live-chat',
                    imageUrl: 'https://a.slack-edge.com/66f9/img/avatars-teams/ava_0001-34.png'
                }}
                msgListScrollRef={msgListScrollRef}
                onGuestSendMessage={onGuestSendMessage.bind(this)}
                messageList={messageList}
                newMessagesCount={newMessagesCount}
                handleClick={handleIconClick.bind(this)}
                isOpen={isOpen}
            />
        </div>
    )
}

export default LauncherWrapper;
