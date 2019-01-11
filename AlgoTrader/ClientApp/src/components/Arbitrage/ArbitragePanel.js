import React, { Component } from 'react';
import './Arbitrage.css';

export class ArbitragePanel extends Component {
    displayName = ArbitragePanel.name

    constructor(props) {
        super(props);

        this.state = {
            error: null,
            isLoaded: false,
            hubConnection: null
        };
    }

    render() {
        const { error, isLoaded, name, symbol, sentiment } = this.state;

        if (error) {
            return <div>Error: {error.message}</div>;
        } else if (!isLoaded) {
            return <div>Loading...</div>;
        } else {
            return (
                <div className="arbitragePanel">
                    Test
                </div>
            );
        }
    }
}