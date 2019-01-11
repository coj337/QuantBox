import React, { Component } from 'react';
import { Col, Row } from 'react-bootstrap';
import { SentimentPanel } from './SentimentPanel';

export class Sentiment extends Component {
  displayName = Sentiment.name

    constructor(props) {
        super(props);

        this.state = {
            hubConnection: null,
            assets: []
        };
    }

    componentDidMount() {
        fetch("/Sentiment/GetSupportedAssets")
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        assets: result
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
                {this.state.assets.map((asset, i) => (
                    <Col xs={6} md={4} lg={3} key={i}>
                        <SentimentPanel name={asset.name} symbol={asset.symbol}/>
                    </Col>
                ))}
            </Row>
        );
    }
}
