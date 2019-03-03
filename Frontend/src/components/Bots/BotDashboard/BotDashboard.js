import React, { Component } from 'react';
import { Row } from 'react-bootstrap';
import { BotPreview } from './BotPreview';
import './BotDashboard.css';

export class BotDashboard extends Component {
    displayName = BotDashboard.name

    constructor(props) {
        super(props);

        this.state = {
            bots: []
        };
    }

    componentDidMount() {
        fetch("/Bot/All")
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        bots: result
                    });
                },
                (error) => {
                    console.log(error);
                }
            );
    }

    render() {
        return (
            <Row>
                <Row>
                    <h3>Bot Dashboard</h3>
                </Row>

                {this.state.bots.map((bot, i) => (
                    <BotPreview
                        key={i}
                        name={bot.name}
                        profit={bot.profit}
                        enabled={bot.tradingEnabled}
                    />
                ))}

                <a href="/bots/new" id="newBotButton" className="darkerContainer">Create New Bot</a>
            </Row>
        );
    }
}