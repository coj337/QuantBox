import React, { Component } from 'react';
import { Col, Row } from 'react-bootstrap';
import { ResponsivePie } from 'nivo';
import { AssetBreakdown } from './AssetBreakdown';
import './PortfolioOverview.css';

export class PortfolioOverview extends Component {
    displayName = PortfolioOverview.name

    constructor(props) {
        super(props);

        this.state = {
            assets: []
        };
    }

    render() {
        const data = [
            {
                "id": "Tron",
                "label": "TRX",
                "value": 10,
                "color": "hsl(198, 70%, 50%)"
            },
            {
                "id": "Ripple",
                "label": "XRP",
                "value": 10,
                "color": "hsl(33, 70%, 50%)"
            },
            {
                "id": "Ethereum",
                "label": "ETH",
                "value": 20,
                "color": "hsl(169, 70%, 50%)"
            },
            {
                "id": "Bitcoin",
                "label": "BTC",
                "value": 40,
                "color": "hsl(184, 70%, 50%)"
            },
            {
                "id": "Stellar",
                "label": "XLM",
                "value": 20,
                "color": "hsl(333, 70%, 50%)"
            }
        ];

        return (
            <Row className="portfolioOverview darkerContainer">
                <Col xs={12}><h4>Overview</h4></Col>
                <Col xs={4} id="breakdownChart">
                    <ResponsivePie
                        data={data}
                        margin={{
                            "top": 40,
                            "right": 0,
                            "bottom": 40,
                            "left": 0
                        }}
                        innerRadius={0.8}
                        radialLabelsTextColor="white"
                        radialLabelsLinkOffset={0}
                        radialLabelsLinkColor="inherit"
                        animate={true}
                        motionStiffness={90}
                        motionDamping={15}
                    />
                </Col>
                <Col xs={8}>
                    <AssetBreakdown currency="Bitcoin" symbol="BTC" />
                    <AssetBreakdown currency="Ethereum" symbol="ETH" />
                    <AssetBreakdown currency="Ripple" symbol="XRP" />
                    <AssetBreakdown currency="Stellar" symbol="XLM" />
                    <AssetBreakdown currency="Tron" symbol="TRX" />
                </Col>
            </Row>
        );
    }
}
