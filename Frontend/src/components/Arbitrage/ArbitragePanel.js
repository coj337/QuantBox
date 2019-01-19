import React, { Component } from 'react';
import './Arbitrage.css';

export class ArbitragePanel extends Component {
    displayName = ArbitragePanel.name

    constructor(props) {
        super(props);

        this.state = {
            error: null,
            isLoaded: false,
            pair: props.pair
        };
    }

    render() {
        const { error, isLoaded } = this.state;

        if (error) {
            return <div className="arbitragePanel">Error: {error.message}</div>;
        } else if (!isLoaded) {
            return <div className="arbitragePanel">Loading...</div>;
        } else {
            return (
                <div className="arbitragePanel">
                    Test
                </div>
            );
        }
    }
}