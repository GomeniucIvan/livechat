import PropTypes from 'prop-types';
import React, { Component } from 'react';
import ChatWindow from './ChatWindow';
import launcherIcon from './assets/logo-no-bg.svg';
import launcherIconActive from './assets/close-icon.png';
import './assets/scss/launcher.scss';

class Launcher extends Component {
  constructor() {
    super();
    this.state = {
      launcherIcon,
      isOpen: false
    };
  }

  handleClick() {
    if (this.props.handleClick !== undefined) {
      this.props.handleClick();
    } else {
      this.setState({
        isOpen: !this.state.isOpen,
      });
    }
  }

  render() {
    const isOpen = this.props.hasOwnProperty('isOpen') ? this.props.isOpen : this.state.isOpen;
    const classList = [
      'app-launcher',
      (isOpen ? 'opened' : ''),
    ];
    return (
      <div>
        <div>
        </div>
        <div className={classList.join(' ')} onClick={this.handleClick.bind(this)}>
          <MessageCount count={this.props.newMessagesCount} isOpen={isOpen} />
                <img className={"app-open-icon"} src={launcherIconActive} />
                <img className={"app-closed-icon"} src={launcherIcon} />
        </div>
        <ChatWindow
          messageList={this.props.messageList}
          onUserInputSubmit={this.props.onMessageWasSent}
          agentProfile={this.props.agentProfile}
          isOpen={isOpen}
          onClose={this.handleClick.bind(this)}
        />
      </div>
    );
  }
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

Launcher.defaultProps = {
  newMessagesCount: 0
}

export default Launcher;
