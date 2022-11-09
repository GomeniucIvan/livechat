import React  from 'react';

const TextMessage = (props) => {
    return (
        <div className="app-message-text">
            <span dangerouslySetInnerHTML={{ __html: props.Message }}></span>
        </div>
    )
}

export default TextMessage