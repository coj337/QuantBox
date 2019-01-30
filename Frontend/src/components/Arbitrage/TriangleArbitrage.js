import React, { Component } from 'react';
import { Col, Row } from 'react-bootstrap';
import { ArbitragePanel } from './ArbitragePanel';

export class TriangleArbitrage extends Component {
    displayName = TriangleArbitrage.name

    constructor(props) {
        super(props);

        this.state = {
            bestResult: null,
            worstResult: null,
            arbResults: []
        };
    }

    componentDidMount() {
        this.getItems();
        this.timer = setInterval(() => this.getItems(), 60 * 1000); //Polling until I get websockets to work
    }

    componentWillUnmount() {
        clearInterval(this.timer);
        this.timer = null;
    }

    getItems() {
        fetch("/Arbitrage/GetTriangleResults")
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        arbResults: result
                    });
                },
                (error) => {
                    console.log(error);
                }
        );

        fetch("/Arbitrage/GetBestTriangleResult")
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

        fetch("/Arbitrage/GetWorstTriangleResult")
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
                <Row>
                    <h3>Triangle Arbitrage</h3>
                </Row>
                <Col xs={6}>
                    <h4 className="center">Best</h4>
                    {this.state.bestResult == null ?
                        <div className="darkerContainer">Loading...</div> :
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
                    <h4 className="center">Worst</h4>
                    {this.state.worstResult == null ?
                        <div className="darkerContainer">Loading...</div> :
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

                <h4 className="subTitle">All Markets</h4>
                {this.state.arbResults.map((arbitrage, i) => (
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
