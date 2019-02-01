import React, { Component } from 'react';
import { Col, Row } from 'react-bootstrap';
import { ArbitragePanel } from './ArbitragePanel';

export class NormalArbitrage extends Component {
    displayName = NormalArbitrage.name

    constructor(props) {
        super(props);

        this.state = {
            bestResult: null,
            worstResult: null,
            arbResults: [],
            lastUpdate: null
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
        fetch("/Arbitrage/GetNormalResults")
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        arbResults: result,
                    });
                    if (result.length != 0) {
                        this.setState({
                            lastUpdate: new Date().toLocaleTimeString()
                        });
                    }
                },
                (error) => {
                    console.log(error);
                }
            );

        fetch("/Arbitrage/GetBestNormalResult")
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

        fetch("/Arbitrage/GetWorstNormalResult")
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
                    <h3>Normal Arbitrage</h3>
                    <p className="right m-r-30">Last Update {this.state.lastUpdate}</p>
                </Row>
                <Col xs={6}>
                    <h4 className="center">Best</h4>
                    {this.state.bestResult == null ?
                        <div className="darkerContainer">Loading...</div> :
                        <ArbitragePanel
                            exchange={this.state.bestResult.exchanges.join(" -> ")}
                            path={this.state.bestResult.pairs.map(function (x) {
                                return x.altCurrency + "/" + x.baseCurrency
                            }).join(" -> ")}
                            transactionFee={this.state.bestResult.transactionFee}
                            profit={parseFloat((this.state.bestResult.profit).toFixed(4))}
                        />
                    }
                </Col>
                <Col xs={6}>
                    <h4 className="center">Worst</h4>
                    {this.state.worstResult == null ?
                        <div className="darkerContainer">Loading...</div> :
                        <ArbitragePanel
                            exchange={this.state.worstResult.exchanges.join(" -> ")}
                            path={this.state.worstResult.pairs.map(function (x) {
                                return x.altCurrency + "/" + x.baseCurrency
                            }).join(" -> ")}
                            transactionFee={this.state.worstResult.transactionFee}
                            profit={parseFloat((this.state.worstResult.profit).toFixed(4))}
                        />
                    }
                </Col>

                <h4 className="subTitle">All Markets</h4>
                {this.state.arbResults.map((arbitrage, i) => (
                    <Col xs={3} key={i}>
                        <ArbitragePanel
                            exchange={arbitrage.exchanges.join(" -> ")}
                            path={arbitrage.pairs.map(function (x) {
                                return x.altCurrency + "/" + x.baseCurrency
                            }).join(" -> ")}
                            transactionFee={arbitrage.transactionFee}
                            profit={parseFloat((arbitrage.profit).toFixed(4))}
                        />
                    </Col>
                ))}
            </Row>
        );
    }
}
