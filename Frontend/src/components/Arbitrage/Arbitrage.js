import React, { Component } from 'react';
import { Col, Row } from 'react-bootstrap';
import { ArbitragePanel } from './ArbitragePanel';

export class Arbitrage extends Component {
    displayName = Arbitrage.name

    constructor(props) {
        super(props);

        this.state = {
            triangleArbResults: [],
            bestResult: null,
            worstResult: null,
            normalArbMatrix: []
        };
    }

    componentDidMount() {
        this.timer = setInterval(() => this.getItems(), 10 * 1000); //Polling until I get websockets to work
    }

    componentWillUnmount() {
        clearInterval(this.timer);
        this.timer = null;
    }

    getItems() {
        //fetch("/Arbitrage/GetTriangleResults")
        fetch("/Arbitrage/GetCurrentResults")
            .then(res => res.json())
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
            .then(res => res.json())
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
            .then(res => res.json())
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
                <h2>Triangle Arbitrage Opportunities</h2>
                <Col xs={6}>
                    <h4>Best</h4>
                    {this.state.bestResult == null ?
                        <div>Loading...</div> :
                        <ArbitragePanel
                            exchange={this.state.bestResult.exchange}
                            path={this.state.bestResult.path}
                            transactionFee={this.state.bestResult.transactionFee}
                            networkFee={this.state.bestResult.networkFee}
                            profit={parseFloat((this.state.bestResult.profit).toFixed(4))}
                            timePerLoop={this.state.bestResult.timePerLoop}
                        />
                    }
                </Col>
                <Col xs={6}>
                    <h4>Worst</h4>
                    {this.state.worstResult == null ?
                        <div>Loading...</div> :
                        <ArbitragePanel
                            exchange={this.state.worstResult.exchange}
                            path={this.state.worstResult.path}
                            transactionFee={this.state.worstResult.transactionFee}
                            networkFee={this.state.worstResult.networkFee}
                            profit={parseFloat((this.state.worstResult.profit).toFixed(4))}
                            timePerLoop={this.state.worstResult.timePerLoop}
                        />
                    }
                </Col>

                <h4>All Markets</h4>
                {this.state.triangleArbResults.map((arbitrage, i) => (
                    <Col xs={3} key={i}>
                        <ArbitragePanel
                            exchange={arbitrage.exchange}
                            path={arbitrage.path}
                            transactionFee={arbitrage.transactionFee}
                            networkFee={arbitrage.networkFee}
                            profit={parseFloat((arbitrage.profit).toFixed(4))}
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
