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
                "id": "Cardano",
                "label": "ADA",
                "value": 10,
                "color": "#333333"
            },
            {
                "id": "Ripple",
                "label": "XRP",
                "value": 10,
                "color": "#006097"
            },
            {
                "id": "Ethereum",
                "label": "ETH",
                "value": 20,
                "color": "#3c3c3d"
            },
            {
                "id": "Bitcoin",
                "label": "BTC",
                "value": 30,
                "color": "#f2a900"
            },
            {
                "id": "Litecoin",
                "label": "LTC",
                "value": 10,
                "color": "#d3d3d3"
            },
            {
                "id": "Stellar",
                "label": "XLM",
                "value": 20,
                "color": "#04b5e5"
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
                        radialLabelsLinkColor="inherit"
                        animate={true}
                        sliceLabel={function (e) { return e.value + "%" }}
                    />
                </Col>
                <Col xs={8}>
                    <AssetBreakdown currency="Bitcoin" symbol="BTC" />
                    <AssetBreakdown currency="Ethereum" symbol="ETH" />
                    <AssetBreakdown currency="Ripple" symbol="XRP" />
                    <AssetBreakdown currency="Stellar" symbol="XLM" />
                    <AssetBreakdown currency="Cardano" symbol="ADA" />
                    <AssetBreakdown currency="Litecoin" symbol="LTC" />
                </Col>
            </Row>
        );
    }
}
