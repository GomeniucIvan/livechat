import React  from 'react';

const TextMessage = (props) => {
  return <div className="app-message-text">{props.data.text}</div>
}

export default TextMessage