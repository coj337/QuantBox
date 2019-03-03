import React, { Component } from 'react';
import { Col } from 'react-bootstrap';
import './BotPreview.css';

export class BotPreview extends Component {
    displayName = BotPreview.name

    constructor(props) {
        super(props);

        this.state = {
            name: props.name,
            profit: props.profit,
            enabled: props.enabled
        };
    }

    render() {
        return (
            <a href={"/bot/" + this.state.name} className="botPreview darkerContainer">
                <h4 className="botTitle">{this.state.name}</h4>
                <Col xs={6}>{this.state.enabled ? "Active" : "Disabled"}</Col>
                <Col xs={6}>{this.state.profit}%</Col>
            </a>
        );
    }
}