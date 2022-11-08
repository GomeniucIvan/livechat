import React, {useEffect, useState} from 'react'
import TestArea from './TestArea';
import Header from './Header';
import Footer from './Footer';
import monsterImgUrl from "./assets/monster.png";
import { postLauncher } from "../utils/HttpClient";
import './assets/styles'

const Home = (props) => {
    let [messageList, setMessageList] = useState([]);
    let [newMessagesCount, setNewMessagesCount] = useState(0);
    let [isOpen, setIsOpen] = useState(false);
    let [loading, setLoading] = useState(true);

    useEffect(() => {
        const PopulateComponent = async () => {

            let response = await postLauncher(`/api/launcher/messages`);

            if (response && response.IsValid) {
                setMessageList(response.Data);
            }

            setLoading(false);
        }
        PopulateComponent();
    });

    const _onMessageWasSent = async (message) => {
        let messages = [...messageList, message];
        setNewMessagesCount(messages);
    }

    const _sendMessage = async (text) => {
        if (text.length > 0) {
            const newMessagesCount = isOpen ? newMessagesCount : newMessagesCount + 1
            this.setState({
                newMessagesCount: newMessagesCount,
                messageList: [...messageList, {
                    author: 'them',
                    type: 'text',
                    data: { text }
                }]
            })
        }
    }

    const handleClick = async () => {
        setIsOpen(true);
        setNewMessagesCount(0);
    }

    return (
        <div>
            <Header />
            <TestArea onMessage={_sendMessage.bind(this)}/>
            <img className="demo-monster-img" src={monsterImgUrl} />
            <Footer />
        </div>
    )
}

export default Home;