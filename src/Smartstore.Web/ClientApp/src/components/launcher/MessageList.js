import React, { useEffect, useRef } from 'react';
import Message from '../launcher/Messages/Message'

const MessageList = (props) => {
    return (
        <div className="app-message-list" ref={props.msgListScrollRef}>
            {props.messages.map((message, i) => {
                return <Message message={message} key={i} />
            })}
        </div>
    )
}

export default MessageList;