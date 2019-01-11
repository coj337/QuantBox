import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Sentiment } from './components/Sentiment/Sentiment';
import { Arbitrage } from './components/Arbitrage/Arbitrage';

export default class App extends Component {
  displayName = App.name

  render() {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/sentiment' component={Sentiment} />
        <Route path='/arbitrage' component={Arbitrage} />
      </Layout>
    );
  }
}
