import { DatabaseDocument } from "../../queries/databases.generated";
import { Skeleton, Result, Card } from "antd";
import { useQuery } from "@apollo/client/react";
import { useQueryHandler } from "../../lib/hooks/useQueryHandler";
import DatabaseSummary from "./DatabaseSummary";
import QueryToolbar from "../QueryToolbar";

export default function Database() {
  const queryVariables = {};
  const { loading, error, data } = useQuery(DatabaseDocument, {
    variables: queryVariables,
  });
  useQueryHandler({ error });
  const database = data?.database;

  if (loading) {
    return <Skeleton active avatar title />;
  }

  if (!database) {
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
        <DatabaseSummary entity={database} />
      </Card>
      <QueryToolbar query={DatabaseDocument} variables={queryVariables} />
    </div>
  );
}
