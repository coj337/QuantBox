import React, { Component } from 'react';
import { Col, Row } from 'react-bootstrap';
import { ArbitragePanel } from './ArbitragePanel';

export class Arbitrage extends Component {
    displayName = Arbitrage.name

    constructor(props) {
        super(props);

        this.state = {
            triangleArbMatrix: [],
            bestResult: "None yet",
            worstResult: "None yet",
            normalArbMatrix: []
        };
    }

    componentWillMount() {
        fetch("/Arbitrage/GetTriangleResults")
            .then(res => {
                return res.json();
            })
            .then(
                (result) => {
                    this.setState({
                        triangleArbResults: result
                    });
                },
                (error) => {
                    console.log(error);
                }
        );

        fetch("/Arbitrage/GetBestResult")
            .then(res => {
                return res.json();
            })
            .then(
                (result) => {
                    this.setState({
                        bestResult: result
                    });
                },
                (error) => {
                    console.log(error);
                }
        );

        fetch("/Arbitrage/GetWorstResult")
            .then(res => {
                return res.json();
            })
            .then(
                (result) => {
                    this.setState({
                        worstResult: result
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
                <h4>Triangle Arbitrage Opportunities</h4>
                <div>Best: {this.state.bestResult}</div>
                <div>Worst: {this.state.worstResult}</div>
                {this.state.triangleArbMatrix.map((arbitrage, i) => (
                    <Col xs={6} md={4} lg={3} key={i}>
                        <ArbitragePanel
                            exchange={arbitrage.exchange}
                            path={arbitrage.path}
                            transactionFee={arbitrage.transactionFee}
                            networkFee={arbitrage.networkFee}
                            profit={arbitrage.profit}
                            timePerLoop={arbitrage.timePerLoop}
                        />
                    </Col>
                ))}

                {/*<div>Two-Way Arbs</div>
                {this.state.normalArbMatrix.map((arbitrage, i) => (
                    <Col xs={6} md={4} lg={3} key={i}>
                        <ArbitragePanel
                            exchange={arbitrage.exchange}
                            path={arbitrage.path}
                            transactionFee={arbitrage.transactionFee}
                            networkFee={arbitrage.networkFee}
                            timePerLoop={arbitrage.timePerLoop}
                        />
                    </Col>
                ))}*/}
            </Row>
        );
    }
}
