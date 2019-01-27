import React, { Component } from 'react';
import './PortfolioChart.css';

export class PortfolioChart extends Component {
    displayName = PortfolioChart.name

    constructor(props) {
        super(props);

        this.state = {
            assets: []
        };
    }

    render() {
        return (
            <div id="chartContainer darkerContainer">
                
            </div>
        );
    }
}
