import React, { Component } from 'react'


const EmojiMessage = (props) => {
  return <div className="app-message-emoji">{props.data.emoji}</div>
}

export default EmojiMessage