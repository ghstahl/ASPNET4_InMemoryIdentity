let actions = {
  addComment: function(record) {
    return {
      type: 'ADD_COMMENT',
      record: record
    }
  },
  addComments: function(records) {
    return {
      type: 'ADD_COMMENTS',
      records: records
    }
  },
  addTableData: function(records) {
      return {
          type: 'ADD_TABLEDATA',
          records: records
      }
  } 
}

export default actions
