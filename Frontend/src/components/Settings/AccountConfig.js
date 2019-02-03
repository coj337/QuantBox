import React, { Component } from 'react';
import './AccountConfig.css';
import { AccountConfigCell } from './AccountConfigCell';
import { Axios } from 'axios';

export class AccountConfig extends Component {
    displayName = AccountConfig.name

    constructor(props) {
        super(props);

        this.state = {
            name: props.name,
            nickname: props.nickname,
            disabled: props.disabled,
            publicKey: props.publicKey,
        };
    }

    componentWillReceiveProps(nextProps) {
        this.setState({
            name: nextProps.name,
            nickname: nextProps.nickname,
            disabled: nextProps.disabled,
            publicKey: nextProps.publicKey,
        });
    }

    render() {
        return (
            <div className={`accountConfig${this.state.disabled ? " disabled" : ""}`}>
                <span className="accountNickname">{this.state.nickname}</span>
                <span className="accountName">{this.state.name}</span>
                <span>
                    <span className="keyContainers">
                        <AccountConfigCell private={false} active={this.state.active} text={this.state.publicKey} id={`${this.state.name}PublicKeyInput`}/>
                        <AccountConfigCell private={true} active={this.state.active} id={`${this.state.name}PrivateKeyInput`} />
                    </span>
                    <button className="darkButton" onClick={() => this.props.onDelete(this.state.name, this.state.publicKey)}>Delete</button>
                </span>
            </div>
        );
    }
}