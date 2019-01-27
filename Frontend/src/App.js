import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Sentiment } from './components/Sentiment/Sentiment';
import { TriangleArbitrage } from './components/Arbitrage/TriangleArbitrage';
import { NormalArbitrage } from './components/Arbitrage/NormalArbitrage';
import { Settings } from './components/Settings/Settings';
import { Portfolio } from './components/Portfolio/Portfolio';

export default class App extends Component {
  displayName = App.name

  render() {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/sentiment' component={Sentiment} />
        <Route path='/arbitrage/normal' component={NormalArbitrage} />
        <Route path='/arbitrage/triangle' component={TriangleArbitrage} />
        <Route path='/portfolio' component={Portfolio} />
        <Route path='/settings' component={Settings} />
      </Layout>
    );
  }
}
