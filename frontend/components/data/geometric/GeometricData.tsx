import { Scalars } from "../../../__generated__/graphql";
import { GeometricDataDocument } from "../../../queries/data.generated";
import { Skeleton, Result, Descriptions } from "antd";
import { useEffect } from "react";
import { messageApolloError } from "../../../lib/apollo";
import DataPageHeader from "../DataPageHeader";
import { useQuery } from "@apollo/client/react";

export type GeometricDataProps = {
    geometricDataId: Scalars["Uuid"]["input"];
};

export default function GeometricData({ geometricDataId }: GeometricDataProps) {
    const { loading, error, data } = useQuery(GeometricDataDocument, {
        variables: {
            uuid: geometricDataId,
        },
    });

    const geometricData = data?.geometricData;


    useEffect(() => {
        if (error) {
            messageApolloError(error);
        }
    }, [error]);

    if (loading) {
        return <Skeleton active avatar title />;
    }

    if (!geometricData) {
        return (
            <Result
                status="500"
                title="500"
                subTitle="Sorry, something went wrong."
            />
        );
    }

    return (
        <DataPageHeader
            data={geometricData}
        // extra={[
        //   <UpdateGeometricData
        //     key="updateGeometricData"
        //     geometricDataId={geometricData.uuid}
        //   />,
        // ]}
        >
            <Descriptions.Item key="thicknesses" label="Thicknesses">
                {geometricData.thicknesses.map((x) => x.toLocaleString("en")).join(", ")}
            </Descriptions.Item>
        </DataPageHeader>
    );
}