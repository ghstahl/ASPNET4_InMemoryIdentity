import React, { Component } from 'react'
import CommentInput from './CommentInput'
import CommentList from './CommentList'
import { connect } from 'react-redux'
import {fetchPosts} from '../actions/Posts'
import actions from '../redux/actions'

import SkyLight from 'react-skylight';
import cx from 'classnames';
var Table = require('reactabular').Table;
class App extends Component {
    constructor(props) {
        super(props);
        this.state = {
      
            columns: [
                {
                    property: 'name',
                    header: 'Name',
                    headerClass: cx({'name-column': true}), // custom props
                },
                {
                    property: 'type',
                    header: 'Type',
                },
                {
                    property: 'description',
                    header: 'Description',
                },
                {
                    property: 'followers',
                    header: 'Followers',
                    // accuracy per hundred is enough for demoing
                    cell: (followers) => followers - (followers % 100),
                },
                {
                    property: 'worksWithReactabular',
                    header: '1st Class Reactabular',
                    // render utf ok if works
                    cell: (works) => works && <span>&#10003;</span>,
                }
            ],
        };
    }
    
  
   

    componentDidMount() {
        this.loadTableDataFromServer();
    }
  
 handleCommentSubmit(comment) {
    var comments = this.state.data;
    // Optimistically set an id on the new comment. It will be replaced by an
    // id generated by the server. In a production application you would likely
    // not use Date.now() for this and would have a more robust system in place.
    comment.id = Date.now();
    var newComments = comments.concat([comment]);
    this.setState({data: newComments});
    $.ajax({
      url: '/api/HelloReactJS/comments',
      dataType: 'json',
      type: 'POST',
      data: comment,
      success: function(data) {
        this.setState({data: data});
      }.bind(this),
      error: function(xhr, status, err) {
        this.setState({data: comments});
        console.error(this.props.url, status, err.toString());
      }.bind(this)
    });
  }

loadTableDataFromServer() {
    $.ajax({
        url: '/api/identityadmin',
      dataType: 'json',
      cache: false,
      success: function(data) {
          this.props.dispatch(actions.addTableData(data));
      }.bind(this),
      error: function(xhr, status, err) {
        console.error(this.props.url, status, err.toString());
      }.bind(this)
    });
  }
render() {
    var columns = this.state.columns || [];
    var data = this.props.data || [];

      return (
      
      <div>
       hi I am the app
       <section>
          <h1>React SkyLight</h1>
          <button onClick={() => this.refs.simpleDialog.show()}>Open Modal</button>
        </section>
        <SkyLight hideOnOverlayClicked ref="simpleDialog" title="Hi, I'm a simple modal">
          Hello, I dont have any callback.
        </SkyLight>
            <Table className='pure-table pure-table-striped'
          columns={columns}
          data={data}
          rowKey='id' />
       <CommentInput dispatch={this.props.dispatch}/>
       <CommentList comments={this.props.comments}/>
      </div>
    )
  }

}
function mapStateToProps(state){
	return state;
}
export default connect(mapStateToProps)(App)
