import React, { Component } from 'react';
import closeIcon from './assets/close-icon.png';


class Header extends Component {

  render() {
    return (
      <div className="app-header">
        <img className="app-header-image" src={this.props.imageUrl} alt="" />
        <div className="app-header-member"> {this.props.teamName} </div>
        <div className="app-header-close-btn" onClick={this.props.onClose}>
          <img src={closeIcon} alt="" />
        </div>
      </div>
    );
  }
}

export default Header;
