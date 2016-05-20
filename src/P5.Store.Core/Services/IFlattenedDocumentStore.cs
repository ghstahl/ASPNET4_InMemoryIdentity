using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P5.Store.Core.Models;

namespace P5.Store.Core.Services
{


    // Summary:
    //     Represents the result of a paged query, returned by manually paged query
    //     executions.

    // Summary:
    //     The interface for a all purpose flattened document store
    //
    // Type parameters:
    //
    public interface IFlattenedDocumentStore
    {
        // Summary:
        //     Retrieves all data for a document identifier.
        //
        // Parameters:
        //   documentType:
        //     The document type.
        //
        // Returns:
        //     A list of token metadata
        Task<IEnumerable<IDocumentRecord>> GetAllByTypeAsync(string documentType);

        // Summary:
        //     Retrieves the one recrod for a document identifier.
        //
        // Parameters:
        //   documentType:
        //     The document type.
        //   documentVersion:
        //     The document version.
        //
        // Returns:
        //     A list of token metadata
        Task<IDocumentRecord> GetByTypeAndVersionAsync(string documentType, string documentVersion);

        // Summary:
        //     Retrieves the data.
        //
        // Parameters:
        //   id:
        //     The id.
        Task<IDocumentRecord> GetByIdAsync(Guid id);

        //
        // Summary:
        //     Removes the data.
        //
        // Parameters:
        //   id:
        //     The id.
        Task RemoveByIdAsync(Guid id);

        //
        // Summary:
        //     Removes ALL the data of this type.
        //
        // Parameters:
        //   documentType:
        //     The document type.
        Task RemoveAllByTypeAsync(string documentType);

        //
        // Summary:
        //     Removes the record of this type and version.
        //
        // Parameters:
        //   documentType:
        //     The document type.
        //
        //   documentVersion:
        //     The document version.
        Task RemoveByTypeAndVersionAsync(string documentType, string documentVersion);

        //
        // Summary:
        //     Stores the data.
        //
        // Parameters:
        //
        //   documentRecord:
        //     The Document Record.
        Task StoreAsync(IDocumentRecord documentRecord);

        //
        // Summary:
        //     Stores the data.
        //
        // Parameters:
        //
        //   documentRecords:
        //     The Document Records.
        Task StoreManyAsync(IEnumerable<IDocumentRecord> documentRecords);

        //
        // Summary:
        //    returns results page request.
        //    1. set pageState to null for the first
        //    2. following calls will set the pageState by using the IPage.PagingState from a previous call
        //
        // Parameters:
        //
        //   documentType:
        //     The document type.
        //   pageSize:
        //     The number of record to return.
        //   pagingState:
        //     a token that keeps track of where we are for the subsequent calls.
        Task<IPage<DocumentRecord>> PageGetByTypeAsync(string documentType, int pageSize, byte[] pagingState);

        //
        // Summary:
        //    returns results page request.
        //    1. set pageState to null for the first
        //    2. following calls will set the pageState by using the IPage.PagingState from a previous call
        //
        // Parameters:
        //
        //   pageSize:
        //     The number of record to return.
        //   pagingState:
        //     a token that keeps track of where we are for the subsequent calls.
        Task<IPage<DocumentRecord>> PageGetAsync(int pageSize, byte[] pagingState);


    }
}