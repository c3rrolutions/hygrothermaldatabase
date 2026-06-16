import { Scalars } from "../../../__generated__/graphql";
import { PhotovoltaicDataDocument } from "../../../queries/data.generated";
import { Skeleton, Result, Card } from "antd";
import { useQuery } from "@apollo/client/react";
import { useQueryHandler } from "../../../lib/hooks/useQueryHandler";
import PhotovoltaicDataSummary from "./PhotovoltaicDataSummary";
import QueryToolbar from "../../QueryToolbar";

interface PhotovoltaicDataProps {
  id: Scalars["Uuid"]["input"];
}

export default function PhotovoltaicData({ id }: PhotovoltaicDataProps) {
  const queryVariables = {
    id,
  };
  const { loading, error, data } = useQuery(PhotovoltaicDataDocument, {
    variables: queryVariables,
  });
  useQueryHandler({ error });
  const theData = data?.data;

  if (loading) {
    return <Skeleton active avatar title />;
  }

  if (!theData) {
    return (
      <Result
        status="500"
        title="500"
        subTitle="Sorry, something went wrong."
      />
    );
  }

  return (
    <div>
      <Card style={{ marginBottom: "1em" }}>
        <PhotovoltaicDataSummary entity={theData} />
      </Card>
      <QueryToolbar
        query={PhotovoltaicDataDocument}
        variables={queryVariables}
      />
    </div>
  );
}
