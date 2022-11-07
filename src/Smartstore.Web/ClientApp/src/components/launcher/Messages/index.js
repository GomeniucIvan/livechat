import React, { Component } from 'react'
import TextMessage from './TextMessage'
import EmojiMessage from './EmojiMessage'
import chatIconUrl from './../assets/chat-icon.svg'


class Message extends Component {
    _renderMessageOfType(type) {
        return <TextMessage {...this.props.message} />

    //switch(type) {
    //  case 'text':
        
    //  case 'emoji':
    //    return <EmojiMessage {...this.props.message} />
    //}
  }

  render () {
    let contentClassList = [
      "app-message-content",
      (this.props.message.author === "me" ? "sent" : "received")
    ];
    return (
      <div className="app-message">
        <div className={contentClassList.join(" ")}>
          <div className="app-message-avatar" style={{
            backgroundImage: `url(${chatIconUrl})`
          }}></div>
          {this._renderMessageOfType(this.props.message.type)}
        </div>
      </div>)
  }
}

export default Message