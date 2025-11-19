import { Descriptions, Typography } from "antd";
import { PageHeader } from "@ant-design/pro-layout";
import Link from "next/link";
import paths from "../../paths";
import { DataPartialFragment } from "../../queries/data.generated";
import { ReactNode } from "react";

export type DataPageHeaderProps = {
    data: DataPartialFragment,
    extra?: ReactNode
    children?: ReactNode
};

export default function DataPageHeader({ data, extra, children }: DataPageHeaderProps) {
    return (
        <PageHeader
            title={data.name}
            subTitle={data.description}
            extra={extra}
            backIcon={false}
        >
            <Descriptions size="small" column={1}>
                <Descriptions.Item label="ID">{data.uuid}</Descriptions.Item>
                <Descriptions.Item label="Timestamp">{data.timestamp}</Descriptions.Item>
                <Descriptions.Item label="Locale">{data.locale}</Descriptions.Item>
                <Descriptions.Item label="Component">
                    <Typography.Link
                        href={paths.metabase.component(data.componentId)}
                    >
                        {data.componentId}
                    </Typography.Link>
                </Descriptions.Item>
                <Descriptions.Item label="Created At">{data.createdAt}</Descriptions.Item>
                <Descriptions.Item label="Creator">
                    <Link href={paths.metabase.institution(data.creatorId)} legacyBehavior>
                        {data.creatorId}
                    </Link>
                </Descriptions.Item>
                <Descriptions.Item label="Warnings">{data.warnings}</Descriptions.Item>
                <Descriptions.Item label="Applied Method">
                    <Typography.Link
                        href={paths.metabase.method(data.appliedMethod.methodId)}
                    >
                        {data.appliedMethod.methodId}
                    </Typography.Link>
                </Descriptions.Item>
                <Descriptions.Item label="Root Resource">
                    <Typography.Link
                        href={data.resourceTree.root.value.locator}
                    >
                        {data.resourceTree.root.value.uuid}
                    </Typography.Link>
                </Descriptions.Item>
                {data.userId != null &&
                    <Descriptions.Item label="User">
                        <Typography.Link
                            href={paths.metabase.user(data.userId)}
                        >
                            {data.userId}
                        </Typography.Link>
                    </Descriptions.Item>
                }
                {children}
            </Descriptions>
        </PageHeader>
    )
};