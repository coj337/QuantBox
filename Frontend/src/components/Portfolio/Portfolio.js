import React, { Component } from 'react';
import { Col, Row } from 'react-bootstrap';
import { PortfolioOverview } from './PortfolioOverview';
import { PortfolioChart } from './PortfolioChart';

export class Portfolio extends Component {
    displayName = Portfolio.name

    constructor(props) {
        super(props);

        this.state = {
            assets: []
        };
    }

    render() {
        return (
            <div>
                <Row>
                    <h3>Portfolio</h3>
                </Row>
                <PortfolioOverview />
                <PortfolioChart />
            </div>
        );
    }
}
