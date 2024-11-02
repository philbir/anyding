/* eslint-disable */
import { TypedDocumentNode as DocumentNode } from '@graphql-typed-document-node/core';
export type Maybe<T> = T | null;
export type InputMaybe<T> = Maybe<T>;
export type Exact<T extends { [key: string]: unknown }> = { [K in keyof T]: T[K] };
export type MakeOptional<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]?: Maybe<T[SubKey]> };
export type MakeMaybe<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]: Maybe<T[SubKey]> };
export type MakeEmpty<T extends { [key: string]: unknown }, K extends keyof T> = { [_ in K]?: never };
export type Incremental<T> = T | { [P in keyof T]?: P extends ' $fragmentName' | '__typename' ? T[P] : never };
/** All built-in and custom scalars, mapped to their actual values */
export type Scalars = {
  ID: { input: string; output: string; }
  String: { input: string; output: string; }
  Boolean: { input: boolean; output: boolean; }
  Int: { input: number; output: number; }
  Float: { input: number; output: number; }
  /** The `Date` scalar represents an ISO-8601 compliant date type. */
  Date: { input: any; output: any; }
  /** The `DateTime` scalar represents an ISO-8601 compliant date time type. */
  DateTime: { input: any; output: any; }
  /** The `Long` scalar type represents non-fractional signed whole 64-bit numeric values. Long can represent values between -(2^63) and 2^63 - 1. */
  Long: { input: any; output: any; }
  UUID: { input: any; output: any; }
};

export type GeoSearchRadiusInput = {
  latitude?: InputMaybe<Scalars['Float']['input']>;
  longitude?: InputMaybe<Scalars['Float']['input']>;
  radius?: InputMaybe<Scalars['Float']['input']>;
};

export type GetThingsRequestInput = {
  type?: InputMaybe<ThingType>;
};

export type SearchFilterInput = {
  comparer?: InputMaybe<Scalars['String']['input']>;
  field: Scalars['String']['input'];
  value: Scalars['String']['input'];
};

export type SearchRequestInput = {
  facetBy?: InputMaybe<Array<Scalars['String']['input']>>;
  filters?: InputMaybe<Array<SearchFilterInput>>;
  geoRadius?: InputMaybe<GeoSearchRadiusInput>;
  pageNr?: InputMaybe<Scalars['Int']['input']>;
  pageSize?: InputMaybe<Scalars['Int']['input']>;
  queryBy?: InputMaybe<Array<Scalars['String']['input']>>;
  text: Scalars['String']['input'];
};

export enum ThingState {
  Active = 'ACTIVE',
  Archived = 'ARCHIVED',
  Deleted = 'DELETED',
  Draft = 'DRAFT',
  New = 'NEW',
  Processing = 'PROCESSING'
}

export enum ThingType {
  Device = 'DEVICE',
  Document = 'DOCUMENT',
  Face = 'FACE',
  Media = 'MEDIA',
  Person = 'PERSON'
}

export type SearchMediaQueryQueryVariables = Exact<{
  pageSize: Scalars['Int']['input'];
}>;


export type SearchMediaQueryQuery = { __typename?: 'Query', searchMedia: { __typename?: 'SearchResultOfMediaIndex', totalFound?: number | null, totalCount: number, hits: Array<{ __typename?: 'SearchHitOfMediaIndex', vectorDistance?: number | null, geoDistance?: Array<{ __typename?: 'GeoDistance', field: string, distance: number }> | null, document: { __typename?: 'MediaIndex', id: string, preview: string, altitude?: number | null, city?: string | null, country?: string | null, countryCode?: string | null, location?: Array<number> | null, name: string, street?: string | null, orientation?: string | null, placeName?: string | null, faces?: Array<{ __typename?: 'FaceInMediaIndex', faceId: string, personId?: string | null, personName?: string | null, ageInMonths?: number | null }> | null, dateTaken?: { __typename?: 'DateIndex', date: string, timestamp: any } | null } }>, facets: Array<{ __typename?: 'FacetResult', field: string, totalValues: number, values: Array<{ __typename?: 'FacetFieldCount', value: string, count: number }> }> } };


export const SearchMediaQueryDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"searchMediaQuery"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}},"type":{"kind":"NonNullType","type":{"kind":"NamedType","name":{"kind":"Name","value":"Int"}}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"searchMedia"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"request"},"value":{"kind":"ObjectValue","fields":[{"kind":"ObjectField","name":{"kind":"Name","value":"pageSize"},"value":{"kind":"Variable","name":{"kind":"Name","value":"pageSize"}}},{"kind":"ObjectField","name":{"kind":"Name","value":"pageNr"},"value":{"kind":"IntValue","value":"0"}},{"kind":"ObjectField","name":{"kind":"Name","value":"facetBy"},"value":{"kind":"ListValue","values":[{"kind":"StringValue","value":"date_taken.year","block":false}]}},{"kind":"ObjectField","name":{"kind":"Name","value":"text"},"value":{"kind":"StringValue","value":"*","block":false}}]}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"totalFound"}},{"kind":"Field","name":{"kind":"Name","value":"totalCount"}},{"kind":"Field","name":{"kind":"Name","value":"hits"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"vectorDistance"}},{"kind":"Field","name":{"kind":"Name","value":"geoDistance"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"field"}},{"kind":"Field","name":{"kind":"Name","value":"distance"}}]}},{"kind":"Field","name":{"kind":"Name","value":"document"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"faces"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"faceId"}},{"kind":"Field","name":{"kind":"Name","value":"personId"}},{"kind":"Field","name":{"kind":"Name","value":"personName"}},{"kind":"Field","name":{"kind":"Name","value":"ageInMonths"}}]}},{"kind":"Field","name":{"kind":"Name","value":"preview"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"name"},"value":{"kind":"StringValue","value":"Preview_Xxxs","block":false}}]},{"kind":"Field","name":{"kind":"Name","value":"altitude"}},{"kind":"Field","name":{"kind":"Name","value":"city"}},{"kind":"Field","name":{"kind":"Name","value":"country"}},{"kind":"Field","name":{"kind":"Name","value":"countryCode"}},{"kind":"Field","name":{"kind":"Name","value":"dateTaken"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"date"}},{"kind":"Field","name":{"kind":"Name","value":"timestamp"}}]}},{"kind":"Field","name":{"kind":"Name","value":"location"}},{"kind":"Field","name":{"kind":"Name","value":"name"}},{"kind":"Field","name":{"kind":"Name","value":"street"}},{"kind":"Field","name":{"kind":"Name","value":"orientation"}},{"kind":"Field","name":{"kind":"Name","value":"placeName"}}]}}]}},{"kind":"Field","name":{"kind":"Name","value":"facets"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"field"}},{"kind":"Field","name":{"kind":"Name","value":"totalValues"}},{"kind":"Field","name":{"kind":"Name","value":"values"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"value"}},{"kind":"Field","name":{"kind":"Name","value":"count"}}]}}]}}]}}]}}]} as unknown as DocumentNode<SearchMediaQueryQuery, SearchMediaQueryQueryVariables>;