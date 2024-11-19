import type React from "react";
import { useQuery } from "urql";
import { graphql } from "../gql";
import {
	Card,
	Text,
	Modal,
	Container,
	Loader,
	TextInput,
	rem,
} from "@mantine/core";
import { useDebouncedValue } from "@mantine/hooks";
import { IconSearch } from "@tabler/icons-react";
import { Routes, Route, BrowserRouter, Outlet } from "react-router-dom";
import classes from "./Media.page.module.css";
import { LazyLoadImage } from "react-lazy-load-image-component";
import "react-lazy-load-image-component/src/effects/blur.css";
import { useNavigate } from "react-router-dom"; // Import useNavigate

import { useState } from "react";
import MediaDetailsPage from "./MediaDetails.page";

const searchMediaQueryDocument = graphql(/* GraphQL */ `
query searchMediaQuery($pageSize: Int!, $searchText: String!) {
  searchMedia(
    request: { 
      pageSize: $pageSize, 
      pageNr: 0, 
      facetBy: ["date_taken.year"],
      text: $searchText }
  ) {
    totalFound
    totalCount
    searchDuration
    hits {
      vectorDistance,
      geoDistance{
         field
         distance
      }
      document {
        id
        faces {
          faceId
          personId
          personName
          ageInMonths
        }
        preview(name:"Preview_Xxs" )
        altitude
        city
        country
        countryCode
        dateTaken {
          date
          timestamp
        }
        location
        name
        street
        orientation
        placeName
      }
    }
    facets {
      field
      totalValues
      values {
        value
        count
      }
    }
  }
}
`);

const MediaPage: React.FC = () => {
	const [searchText, setSearchText] = useState("");
	const [debounced] = useDebouncedValue(searchText, 500);
	const [result] = useQuery({
		query: searchMediaQueryDocument,
		variables: { pageSize: 100, searchText: debounced },
	});
	const { data, fetching, error } = result;
	const hits = data?.searchMedia?.hits;
	const icon = <IconSearch style={{ width: rem(16), height: rem(16) }} />;
	const navigate = useNavigate(); // Initialize useNavigate
	const [opened, setOpened] = useState(false);

	const handleImageClick = (id: string) => {
		navigate(`/media/${id}`);
	};

	return (
		<>
			<Outlet />

			<Container h={80} m={0}>
				<TextInput
					leftSectionPointerEvents="none"
					leftSection={fetching ? <Loader size="xs" /> : icon}
					placeholder="Search..."
					value={searchText}
					onChange={(event) => setSearchText(event.currentTarget.value)}
				/>
			</Container>
			<Container fluid>
				{error && <div>Error...</div>}
				<div className={classes.imageList}>
					{hits?.map((hit: any) => (
						<LazyLoadImage
							key={hit.document.id}
							alt={hit.document.name}
							placeholderSrc={hit.document.preview}
							src={`/api/things/data/${hit.document.id}/Preview_SqS`}
							effect="blur"
							className={classes.imageItem}
							onClick={() => handleImageClick(hit.document.id)} // Add onClick handler
						/>
					))}
				</div>
			</Container>
			<Container>
				<Text size="sm" align="center">
					Search duration: {data?.searchMedia?.searchDuration} ms
				</Text>
			</Container>
		</>
	);
};

export default MediaPage;
