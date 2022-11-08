import React, { useEffect, useRef } from 'react';
import Message from '../launcher/Messages/Message'

const MessageList = (props) => {
    const scrollRef = useRef(null);

    useEffect(() => {
        setTimeout(() => {
            if (scrollRef.current) {
                scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
            }
        }, 50)
    }, []);

    return (
        <div className="app-message-list" ref={scrollRef}>
            {props.messages.map((message, i) => {
                return <Message message={message} key={i} />
            })}
        </div>
    )
}

export default MessageList;