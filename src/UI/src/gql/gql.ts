/* eslint-disable */
import * as types from './graphql';
import { TypedDocumentNode as DocumentNode } from '@graphql-typed-document-node/core';

/**
 * Map of all GraphQL operations in the project.
 *
 * This map has several performance disadvantages:
 * 1. It is not tree-shakeable, so it will include all operations in the project.
 * 2. It is not minifiable, so the string of a GraphQL query will be multiple times inside the bundle.
 * 3. It does not support dead code elimination, so it will add unused operations.
 *
 * Therefore it is highly recommended to use the babel or swc plugin for production.
 * Learn more about it here: https://the-guild.dev/graphql/codegen/plugins/presets/preset-client#reducing-bundle-size
 */
const documents = {
    "\nquery searchMediaQuery($pageSize: Int!) {\n  searchMedia(\n    request: { \n      pageSize: $pageSize, \n      pageNr: 0, \n      facetBy: [\"date_taken.year\"]\n      text: \"*\" }\n  ) {\n    totalFound\n    totalCount\n    hits {\n      vectorDistance,\n      geoDistance{\n         field\n         distance\n      }\n      document {\n        id\n        faces {\n          faceId\n          personId\n          personName\n          ageInMonths\n        }\n        preview(name:\"Preview_Xxxs\" )\n        altitude\n        city\n        country\n        countryCode\n        dateTaken {\n          date\n          timestamp\n        }\n        location\n        name\n        street\n        orientation\n        placeName\n      }\n    }\n    facets {\n      field\n      totalValues\n      values {\n        value\n        count\n      }\n    }\n  }\n}\n": types.SearchMediaQueryDocument,
};

/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 *
 *
 * @example
 * ```ts
 * const query = graphql(`query GetUser($id: ID!) { user(id: $id) { name } }`);
 * ```
 *
 * The query argument is unknown!
 * Please regenerate the types.
 */
export function graphql(source: string): unknown;

/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function graphql(source: "\nquery searchMediaQuery($pageSize: Int!) {\n  searchMedia(\n    request: { \n      pageSize: $pageSize, \n      pageNr: 0, \n      facetBy: [\"date_taken.year\"]\n      text: \"*\" }\n  ) {\n    totalFound\n    totalCount\n    hits {\n      vectorDistance,\n      geoDistance{\n         field\n         distance\n      }\n      document {\n        id\n        faces {\n          faceId\n          personId\n          personName\n          ageInMonths\n        }\n        preview(name:\"Preview_Xxxs\" )\n        altitude\n        city\n        country\n        countryCode\n        dateTaken {\n          date\n          timestamp\n        }\n        location\n        name\n        street\n        orientation\n        placeName\n      }\n    }\n    facets {\n      field\n      totalValues\n      values {\n        value\n        count\n      }\n    }\n  }\n}\n"): (typeof documents)["\nquery searchMediaQuery($pageSize: Int!) {\n  searchMedia(\n    request: { \n      pageSize: $pageSize, \n      pageNr: 0, \n      facetBy: [\"date_taken.year\"]\n      text: \"*\" }\n  ) {\n    totalFound\n    totalCount\n    hits {\n      vectorDistance,\n      geoDistance{\n         field\n         distance\n      }\n      document {\n        id\n        faces {\n          faceId\n          personId\n          personName\n          ageInMonths\n        }\n        preview(name:\"Preview_Xxxs\" )\n        altitude\n        city\n        country\n        countryCode\n        dateTaken {\n          date\n          timestamp\n        }\n        location\n        name\n        street\n        orientation\n        placeName\n      }\n    }\n    facets {\n      field\n      totalValues\n      values {\n        value\n        count\n      }\n    }\n  }\n}\n"];

export function graphql(source: string) {
  return (documents as any)[source] ?? {};
}

export type DocumentType<TDocumentNode extends DocumentNode<any, any>> = TDocumentNode extends DocumentNode<  infer TType,  any>  ? TType  : never;