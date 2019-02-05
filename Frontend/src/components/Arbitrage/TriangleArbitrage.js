import React, { Component } from 'react';
import { Col, Row } from 'react-bootstrap';
import { ArbitragePanel } from './ArbitragePanel';
import { ArbitrageSettings } from './ArbitrageSettings';
import { ArbitrageTrades } from './ArbitrageTrades';

export class TriangleArbitrage extends Component {
    displayName = TriangleArbitrage.name

    constructor(props) {
        super(props);

        this.state = {
            bestResult: null,
            worstResult: null,
            arbResults: [],
            lastUpdate: null,
            botId: "Triangle Arbitrage"
        };
    }

    componentDidMount() {
        this.getItems();
        this.timer = setInterval(() => this.getItems(), 30 * 1000); //Polling until I get websockets to work
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
                        arbResults: result,
                    });
                    if (result.length !== 0) {
                        this.setState({
                            lastUpdate: new Date().toLocaleTimeString()
                        });
                    }
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
                            exchange={this.state.worstResult.exchanges.join(" -> ")}
                            path={this.state.worstResult.pairs.map(function (x) {
                                return x.altCurrency + "/" + x.baseCurrency
                            }).join(" -> ")}
                            transactionFee={this.state.worstResult.transactionFee}
                            networkFee={this.state.worstResult.networkFee}
                            profit={parseFloat((this.state.worstResult.profit).toFixed(4))}
                            timePerLoop={this.state.worstResult.timePerLoop}
                        />
                    }
                </Col>

                <h4 className="subTitle">Top Markets</h4>
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

                <ArbitrageSettings botId={this.state.botId} />

                <ArbitrageTrades />
            </Row>
        );
    }
}
