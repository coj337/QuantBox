import React, { Component } from 'react';
import './AccountConfigCell.css';

export class AccountConfigCell extends Component {
    displayName = AccountConfigCell.name

    constructor(props) {
        super(props);

        this.state = {
            private: props.private,
            text: props.text,
            id: props.id
        };
    }

    componentWillReceiveProps(nextProps) {
        this.setState({
            active: nextProps.active,
            text: nextProps.text
        });
    }

    render() {
        return (
            <span className="keyContainer">
                {this.state.active ?
                    <input id={this.state.id} className="keyInput" type={this.state.private ? "password" : "text"} text={this.state.text} /> :
                    <span className="keyDisplay">{this.state.private ? "****************" : this.state.text}</span>
                }
            </span>
        );
    }
}