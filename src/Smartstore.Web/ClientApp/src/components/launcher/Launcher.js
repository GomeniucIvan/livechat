import PropTypes from 'prop-types';
import React, { useState } from 'react';
import ChatWindow from './ChatWindow';
import launcherIcon from './assets/logo-no-bg.svg';
import launcherIconActive from './assets/close-icon.png';
import './assets/scss/launcher.scss';

const Launcher = (props) => {
    let [isOpen, setIsOpen] = useState(false);
    let [loading, setLoading] = useState(true);

    const handleClick = async () => {
        setIsOpen(!isOpen);
    }

    const classList = [
        'app-launcher',
        (isOpen ? 'opened' : ''),
    ];

    return (
        <div>
            <div>
            </div>
            <div className={classList.join(' ')} onClick={handleClick.bind(this)}>
                <MessageCount count={props.newMessagesCount} isOpen={isOpen} />
                <img className={"app-open-icon"} src={launcherIconActive} />
                <img className={"app-closed-icon"} src={launcherIcon} />
            </div>
            <ChatWindow
                messageList={props.messageList}
                onUserInputSubmit={props.onMessageWasSent}
                agentProfile={props.agentProfile}
                isOpen={isOpen}
                onClose={handleClick.bind(this)}
            />
        </div>
    );
}

const MessageCount = (props) => {
  if (props.count === 0 || props.isOpen === true) { return null }
  return (
      <div className={"app-new-messsages-count"}>
      {props.count}
    </div>
  )
}

Launcher.propTypes = {
  onMessageWasReceived: PropTypes.func,
  onMessageWasSent: PropTypes.func,
  newMessagesCount: PropTypes.number,
  isOpen: PropTypes.bool,
  handleClick: PropTypes.func,
  messageList: PropTypes.arrayOf(PropTypes.object)
};

export default Launcher;
