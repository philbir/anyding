
import { useQuery } from 'urql'
import { graphql } from '../gql';
import { Card, Text, SimpleGrid, Container, Loader, TextInput, rem } from '@mantine/core';
import { IconSearch } from '@tabler/icons-react';

const searchMediaQueryDocument = graphql(/* GraphQL */ `
query searchMediaQuery($pageSize: Int!) {
  searchMedia(
    request: { 
      pageSize: $pageSize, 
      pageNr: 0, 
      facetBy: ["date_taken.year"]
      text: "Sedrun" }
  ) {
    totalFound
    totalCount
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

export function MediaPage() {

    const [result] = useQuery({ query: searchMediaQueryDocument, variables: { pageSize: 100 } })
    const { data, fetching, error } = result
    const hits = data?.searchMedia?.hits;
    const icon = <IconSearch style={{ width: rem(16), height: rem(16) }} />;
    return (
        <>
            <Container h={80} m={0}>
            <TextInput w={500}
        leftSectionPointerEvents="none"
        leftSection={icon}
        placeholder="Search..."
      />

            </Container>
            <SimpleGrid cols={6} m={0}>
                {fetching && <Loader color="blue" />}
                {error && <div>Error...</div>}
                {hits?.map((hit: any) => (
                    <Card key={hit.document.id} shadow="sm" padding="lg" radius="md" withBorder>
                        <Text fw={500} size="lg" mt="md">
                            {hit.document.name}
                        </Text>
                        <Text mt="xs" c="dimmed" size="sm">
                            {hit.document.city}, {hit.document.country}
                        </Text>
                    </Card>
                ))}
            </SimpleGrid>
        </>
    );
}

export default MediaPage;
