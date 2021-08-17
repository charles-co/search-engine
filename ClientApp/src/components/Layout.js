import React, { Component } from 'react';
import { Col, Grid, Row } from 'react-bootstrap';
import "./Layout.css";

export class Layout extends Component {
  displayName = Layout.name

  render() {
      return (
          <div className="layout">
              <Grid fluid>
                  <Row>
                      <Col sm={12}>
                          {this.props.children}
                      </Col>
                  </Row>
                  <footer>
                      <p>C# group project 2019/2020</p>
                  </footer>
              </Grid>
          </div>
    );
  }
}
